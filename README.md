Development Notes - Addison Freeman

Pagination:
1) Update /books/ endpoint in BooksController.cs and create new method in BookService.cs. Accept page number as URL parameter (default to page 1 if none provided). Include recordsPerPage URL parameter option with default to 10 per page.
2) Set homepage to load first page of books by default in Home.vue and use b-pagination component that re-loads list from fetch of /books?page={index} through the dataContext (itemsProvider) method. Bind props to b-table and b-pagination components [[1]](https://stackoverflow.com/questions/54494452/getting-bootstrap-vue-pagination-to-play-with-rest-api)
4) Send total books count in custom header for all /books endpoints, expose header explicitly using `access-control-expose-headers` because the CORS-all setting is enabled [[2]](https://github.com/pagekit/vue-resource/issues/663) [[3]](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Expose-Headers).


Search:
1) Add query and filter URL parameter to /books endpoint (e.g. ?query=Zend&filter=title).
2) Create helper extension method for filtering books [[4]](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
3) Bind filter params to watch method in View to call item provider method [[5]](https://stackoverflow.com/questions/53119086/how-do-i-update-the-items-async-in-a-b-table-from-bootstrap-vue-reusing-the-item)


Edit Book:
1) introduce edit state bound to texteditable params with cached description for edge case where user edits then cancels [[6]](https://stackoverflow.com/questions/42133894/vue-js-how-to-properly-watch-for-nested-data) [[7]](https://bootstrap-vue.org/docs/components/form-textarea) [[8]](https://stackoverflow.com/questions/48929139/hide-div-onclick-in-vue)
2) watch params to POST to new endpoint as urlformencoded to update record by bookId [[9]](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio)


Create Book:
1) Use b-modal component with simple client-side validation to POST on confirm. 
2) Create book with new endpoint that saves to data context and retrieves new value for HTTP response.

Other:
Testing:
- Use DefaultHttpContext in NuGet to moq web request methods [[10]](https://stackoverflow.com/questions/30909943/how-to-setup-request-header-in-fakehttpcontext-for-unit-testing)

Security:
- npm i: 539 vulnerabilities (370 high!) run audit
- should CORS be set to allow all? requests should be locked to the test/production domains set in env

Performance:
- make sure to use IQueryable methods to prevent loading entire dataset into memory (possibly deprecate /books/ endpoint that loads entire dataset) [[11]](https://dotnetcorecentral.com/blog/ienumerable-vs-iqueryable/

Challenges/Questions:
- Testing methods with httpContext and other network mocked objects proved difficult. I would ask for assistance from a more senior dev to get unblocked on this task.
- The links in this document are resources I found helpful during development.


Thank you for reading! Please reach out if you have any questions!
