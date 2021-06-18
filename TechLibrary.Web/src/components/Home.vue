<template>
    <div class="home">
        <h1>{{ msg }}</h1>

        <b-table striped hover :items="dataContext" :fields="fields" responsive="sm" :per-page="perPage" :current-page="currentPage">
            <template v-slot:cell(thumbnailUrl)="data">
                <b-img :src="data.value" thumbnail fluid></b-img>
            </template>
            <template v-slot:cell(title_link)="data">
                <b-link :to="{ name: 'book_view', params: { 'id' : data.item.bookId } }">{{ data.item.title }}</b-link>
            </template>
        </b-table>
        <b-pagination :total-rows="totalItems" v-model="currentPage" :per-page="perPage" responsive="sm"></b-pagination>
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
            items: []
        }),
        
        methods: {
            // By default, the items provider function is responsible for all 
            // paging, filtering, and sorting of the data, 
            // before passing it to b - table for display.
            dataContext(ctx, callback) {
                // get fields using ctx.currentPage, was previously getting all records
                axios.get(`https://localhost:5001/books?page=${ctx.currentPage}`)
                    .then(response => {
                        var totalBooksCount = response.headers['x-total-books-count'];
                        this.totalItems = totalBooksCount;
                        callback(response.data);
                    });
            }
        },
        watch: {
            currentPage: {
                handler: function (value) {
                    console.log('currentPage', value);
                }
            }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

