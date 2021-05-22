import React, { useState } from "react";

import {
  Container,
  Typography,
  Link,
  Box,
  TextField,
  Grid,
  IconButton,
} from "@material-ui/core";
import { BookCard } from "../../components/book-card";
import { BookMediaCard } from "../../components/book-media-card";
import { useStyles } from "./search-books.styles";
import { SearchTabs } from "./components";
import { BookService } from "services";
import ListIcon from "@material-ui/icons/List";
import ViewComfyIcon from "@material-ui/icons/ViewComfy";
import ViewListIcon from "@material-ui/icons/ViewList";
import SettingsIcon from "@material-ui/icons/Settings";
import ReorderIcon from "@material-ui/icons/Reorder";
import { Pagination } from "@material-ui/lab";
import TablePagination from "@material-ui/core/TablePagination";
import { Book } from "types";

const SearchBooks = () => {
  const classes = useStyles();
  const [isListView, setListView] = useState<boolean>(true);
  console.log("listview: ", isListView);

  const [page, setPage] = React.useState(2);
  const [books, setBooks] = React.useState<Book.Book[]>();
  const [rowsPerPage, setRowsPerPage] = React.useState(10);
  const [totalRecords, setTotalRecords] = React.useState(0);

  const handleChangePage = (
    event: React.MouseEvent<HTMLButtonElement> | null,
    newPage: number
  ) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  return (
    <Container>
      <SearchTabs
        setTotalRecords={setTotalRecords}
        page={page}
        rowsPerPage={rowsPerPage}
        setBooks={setBooks}
      />
      <Grid
        container
        justify="flex-end"
        className={classes.filterIconsContainer}
      >
        <IconButton
          aria-label="delete"
          onClick={() => {
            setListView(true);
          }}
        >
          <ReorderIcon className={classes.filterIcon} />
        </IconButton>
        <IconButton
          aria-label="delete"
          onClick={() => {
            setListView(false);
          }}
          className={classes.filterIcon}
        >
          <ViewComfyIcon />
        </IconButton>
      </Grid>
      {isListView ? (
        books?.map((book) => (
          <Box my={3} key={book.id}>
            <BookCard {...book} />
          </Box>
        ))
      ) : (
        <Grid container spacing={4}>
          {books?.map((book) => (
            <Grid item sm={3} key={book.id}>
              <BookMediaCard {...book} />
            </Grid>
          ))}
        </Grid>
      )}
      <TablePagination
        component="div"
        count={totalRecords}
        page={page}
        onChangePage={handleChangePage}
        rowsPerPage={rowsPerPage}
        onChangeRowsPerPage={handleChangeRowsPerPage}
      />{" "}
    </Container>
  );
};

export { SearchBooks };
