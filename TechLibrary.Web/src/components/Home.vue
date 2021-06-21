<template>
    <div class="home">
        <h1>{{ msg }}</h1>
        <div class="justify-content-left">
            <b-button v-b-modal.add-book-modal>Add New Book</b-button>
            <b-modal id="add-book-modal" title="Add New Book" @ok="saveBook">
                <form ref="form" @submit.stop.prevent="handleSubmit">
                    <b-form-fieldset label="Book Title: ">
                        <b-form-input type="text" class="form-control" v-model="newBookTitle" placeholder="Enter Title"></b-form-input>
                    </b-form-fieldset>
                    <b-form-fieldset label="Book ISBN: ">
                        <b-form-input type="text" class="form-control" v-model="newBookIsbn" placeholder="Enter ISBN"></b-form-input>
                    </b-form-fieldset>
                    <b-form-fieldset label="Book Description: ">
                        <b-form-input type="text" class="form-control" v-model="newBookDescription" placeholder="The summary of this book..."></b-form-input>
                    </b-form-fieldset>
                    <p v-show="addBookError.visible">{{addBookError.message}}</p>
                </form>
            </b-modal>
            <b-form-fieldset label="Filter By:">
                <b-form-select id="filter-by" v-model="filterType" :options="filterTypes"></b-form-select>
            </b-form-fieldset>
            <b-form-fieldset label="Search For: ">
                <b-form-input type="text" class="form-control" placeholder="Search for ..." v-model="filterString"></b-form-input>
            </b-form-fieldset>
            <b-pagination :total-rows="totalItems" v-model="currentPage" :per-page="perPage" responsive="sm"></b-pagination>
            <b-table ref="table" striped hover :no-local-sorting="true" :items="dataContext" :fields="fields" responsive="sm" :per-page="perPage" :current-page="currentPage" :filterString="filterString" :filterType="filterType">
                <template v-slot:cell(thumbnailUrl)="data">
                    <b-img :src="data.value" thumbnail fluid></b-img>
                </template>
                <template v-slot:cell(title_link)="data">
                    <b-link :to="{ name: 'book_view', params: { 'id' : data.item.bookId } }">{{ data.item.title }}</b-link>
                </template>
            </b-table>

        </div>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Home',
        props: {
            msg: String
        },
        data: () => ({
            fields: [
                { key: 'thumbnailUrl', label: 'Book Image' },
                { key: 'title_link', label: 'Book Title', sortable: true, sortDirection: 'desc' },
                { key: 'isbn', label: 'ISBN', sortable: true, sortDirection: 'desc' },
                { key: 'descr', label: 'Description', sortable: true, sortDirection: 'desc' }
            ],
            currentPage: 1,
            // Setting per-page to 0 (default) will disable the local items pagination feature.
            // https://bootstrap-vue.org/docs/components/table#pagination
            perPage: 0,
            // TODO update total items count inside itemsProvider
            totalItems: 0,
            filterString: "",
            filterType: "Title",
            filterTypes: ["Title", "Description"],
            items: [],
            newBookTitle: "",
            newBookDescription: "",
            newBookIsbn: 0,
            addBookError: {
                visible: false,
                message: "Please enter valid values in all fields"
            }
        }),
        
        methods: {
            // By default, the items provider function is responsible for all 
            // paging, filtering, and sorting of the data, 
            // before passing it to b-table for display.
            dataContext(ctx, callback) {
                if (ctx === undefined) {
                    ctx = {};
                    ctx.currentPage = this.currentPage;
                    ctx.filterType = this.filterType;
                    ctx.filterString = this.filterString;
                }
                // get fields using ctx.currentPage, was previously getting all records
                axios.get(`https://localhost:5001/books?page=${ctx.currentPage}&filterType=${this.filterType}&filterString=${this.filterString}`)
                    .then(response => {
                        var totalBooksCount = response.headers['x-total-books-count'];
                        this.totalItems = totalBooksCount;
                        callback(response.data);
                    });
            },
            saveBook(event) {
                // basic validation
                if (this.newBookTitle.trim() === "" || this.newBookDescription.trim() === "" || this.newBookIsbn === 0) {
                    // show error message and stop modal from closing if failed validation
                    console.log('new book empty fields');
                    this.addBookError.visible = true;
                    event.preventDefault();
                } else {
                    axios.post(`https://localhost:5001/books/`, `title=${this.newBookTitle}&description=${this.newBookDescription}&isbn=${this.newBookIsbn.toString()}`)
                        .then(response => {
                            // on successful creation refresh table and reinit default new book props
                            this.book = response.data;
                            console.log('saved book!');
                            this.$refs.table.refresh();
                            this.newBookTitle = "";
                            this.newBookDescription = "";
                            this.newBookIsbn = 0;
                            this.addBookError.visible = false;
                        });
                }
            }
        },
        watch: {
            filterType: {
                handler: function () {
                    // restart pagination 
                    this.currentPage = 1;
                    this.$refs.table.refresh();
                }
            },
            filterString: {
                handler: function () {
                    // restart pagination 
                    this.currentPage = 1;
                    this.$refs.table.refresh();
                }
            }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

