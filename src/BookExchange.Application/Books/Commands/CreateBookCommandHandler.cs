﻿using BookExchange.Application.Books.Events;
using BookExchange.Application.Common.Exceptions;
using BookExchange.Application.Common;
using BookExchange.Domain.Interfaces;
using BookExchange.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookExchange.Application.Books.Commands
{
     class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book>
     {
          private readonly IMediator _mediator;
          private readonly IHostingEnvironment _environment;
          private readonly IBookRepository _bookRepository;
          private readonly IRepositoryBase<Author> _bookAuthorsRepository;
          private readonly IRepositoryBase<BookCategory> _bookCategoriesRepository;

          public CreateBookCommandHandler(IBookRepository bookRepository, 
               IRepositoryBase<Author> bookAuthorsRepository, IRepositoryBase<BookCategory> bookCategoriesRepository, 
               IHostingEnvironment environment, IMediator mediator)
          {
               _bookRepository = bookRepository;
               _bookAuthorsRepository = bookAuthorsRepository;
               _bookCategoriesRepository = bookCategoriesRepository;
               _environment = environment;
               _mediator = mediator;
          }



          public async Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
          {
               if (ServiceUtils.CheckBookWithIsbnExists(_bookRepository, request.ISBN)) {
                    throw new BadRequestException($"Book with ISBN = {request.ISBN} already exists");
               }

               var uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads", "books");
               var thumbnailPath = await ServiceUtils.SaveFile(request.Thumbnail, uploadDirectory);
               var imagePath = await ServiceUtils.SaveFile(request.Image, uploadDirectory);

               var book = new Book
               {
                    Title = request.Title,
                    ISBN = request.ISBN,
                    ShortDescription = request.ShortDescription,
                    ThumbnailPath = thumbnailPath,
                    Authors = request.AuthorsIds?.Select(id => _bookAuthorsRepository.GetById(id)).ToList(),
                    Categories = request.CategoriesIds?.Select(id => _bookCategoriesRepository.GetById(id)).ToList(),
                    Details = new BookDetails
                    {
                         Description = request.Description,
                         Publisher = request.Publisher,
                         PublishedOn = request.PublishedOn,
                         PageCount = request.PageCount,
                         ImagePath = imagePath
                    }
               };

               _bookRepository.Add(book);
               _bookRepository.SaveAll();

               await _mediator.Publish(new BookCreatedEvent {
                                        Id = book.Id,
                                        Title = book.Title,
                                        ShortDescription = book.ShortDescription,
                                        Description = book.Details.Description,
                                        Authors = book.Authors?.Select(a => a.Name).ToList(),
                                        Categories = book.Categories?.Select(a => a.Label).ToList()
                                       }, cancellationToken); 

               return await Task.FromResult(book);
          }




     }
}
