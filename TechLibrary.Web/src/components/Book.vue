<template>
    <div v-if="book">
        <b-card :title="book.title"
                :img-src="book.thumbnailUrl"
                img-alt="Image"
                img-top
                tag="article"
                style="max-width: 30rem;"
                class="mb-2">
            <b-card-text v-show="!edit.allow">
                {{ book.descr }}
            </b-card-text>
            <b-form-textarea v-show="edit.allow"
                             v-model="book.descr"
                             rows="6">
                {{ book.descr }}
            </b-form-textarea>
            <b-button to="/" variant="primary">Back</b-button>
            <b-button variant="info" @click="toggleEditState">{{edit.message}}</b-button>
            <b-button variant="danger" v-show="edit.allow" @click="cancelEditState">Cancel</b-button>
        </b-card>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Book',
        props: ["id"],
        data: () => ({
            book: null,
            edit: {
                allow: false,
                message: "Edit"
            }
        }),
        mounted() {
            axios.get(`https://localhost:5001/books/${this.id}`)
                .then(response => {
                    this.book = response.data;
                });
        },
        methods: {
            toggleEditState: function () {
                this.edit.allow = !this.edit.allow;
                if (!this.edit.allow) {
                    // toggling away from save
                    this.updateDescription();
                }
            },
            cancelEditState: function () {
                this.edit.allow = !this.edit.allow;
                // TODO refresh state on cancel
            },
            updateDescription: function () {
                console.log('update endpoint', this.book);
               
                axios.put(`https://localhost:5001/books/${this.id}`, `description=${this.book.descr}`)
                    .then(response => {
                        this.book = response.data;
                        console.log('updated book!')
                    });
            }
        },
        watch: {
            'edit.allow': function () {
                if (this.edit.allow) {
                    this.edit.message = "Save";
                } else {
                    this.edit.message = "Edit";
                }
            }
        }
    }
</script>