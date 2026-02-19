# Book Management Web Application - Implementation Plan

## 1. PROJECT ARCHITECTURE

```
BookManagementApp/
├── BookAPI/                          # ASP.NET Backend
│   ├── Controllers/
│   │   └── BooksController.cs
│   ├── Models/
│   │   └── Book.cs
│   ├── Services/
│   │   └── BookService.cs
│   ├── Program.cs
│   └── appsettings.json
│
└── book-management-frontend/         # Angular Frontend
    ├── src/
    │   ├── app/
    │   │   ├── components/
    │   │   │   ├── book-list/
    │   │   │   ├── book-form/
    │   │   │   └── book-item/
    │   │   ├── services/
    │   │   │   └── book.service.ts
    │   │   ├── models/
    │   │   │   └── book.model.ts
    │   │   ├── app.component.ts
    │   │   └── app.module.ts
    │   └── assets/
    └── angular.json
```

## 2. FEATURES OVERVIEW

### Core Features:

1. **View Books** - Display all books in a table/card format
2. **Add Book** - Form to create new books
3. **Edit Book** - Modify existing book details
4. **Delete Book** - Remove books from the list
5. **Form Validation** - Ensure data quality
6. **Error Handling** - User-friendly error messages
7. **Responsive Design** - Works on desktop/tablet/mobile

## 3. USER FLOW

```
User Opens App
    ↓
[Book List View] - Displays all books
    ├─→ [View] → See book details
    ├─→ [Edit] → Opens edit form with current data
    │               ↓
    │         [Edit Form] → User updates fields
    │               ↓
    │         [Save] or [Cancel]
    │               ↓
    │         Back to Book List
    │
    ├─→ [Delete] → Remove book (with confirmation)
    │               ↓
    │         List refreshes
    │
    └─→ [Add New Book] → Opens add form
                    ↓
            [Add Form] - Empty form
                    ↓
            User enters: Title, Author, ISBN, Publication Date
                    ↓
            [Submit] or [Cancel]
                    ↓
            Back to Book List (if success)
```

## 4. DATA MODEL

### Book Model

```
Book {
  id: number                          // Auto-generated unique identifier
  title: string                       // Book title (required)
  author: string                      // Book author (required)
  isbn: string                        // ISBN (required, unique)
  publicationDate: Date               // Publication date (required)
}
```

## 5. API ENDPOINTS

### Base URL: `https://localhost:5001/api/books`

| Method | Endpoint          | Purpose         | Request Body | Response         |
| ------ | ----------------- | --------------- | ------------ | ---------------- |
| GET    | `/api/books`      | Get all books   | None         | 200: Book[]      |
| GET    | `/api/books/{id}` | Get single book | None         | 200: Book \| 404 |
| POST   | `/api/books`      | Create new book | Book         | 201: Book \| 400 |
| PUT    | `/api/books/{id}` | Update book     | Book         | 200: Book \| 404 |
| DELETE | `/api/books/{id}` | Delete book     | None         | 204 \| 404       |

## 6. IMPLEMENTATION PHASES

### Phase 1: Setup & Project Structure (30 mins)

- Create ASP.NET Core Web API project
- Create Angular project
- Configure CORS for frontend-backend communication
- Set up project folders and files

### Phase 2: Backend Development (1-1.5 hours)

1. Create Book model with properties
2. Create BookService with in-memory storage
3. Create BooksController with all CRUD endpoints
4. Add input validation
5. Test API endpoints using Postman/Thunder Client

### Phase 3: Frontend Development (1.5-2 hours)

1. Create Book model/interface
2. Create BookService to call backend API
3. Develop Components:
   - BookListComponent (display all books)
   - BookFormComponent (add/edit books)
   - BookItemComponent (single book display)
4. Add routing between components
5. Add styling (CSS/Bootstrap)
6. Implement form validation

### Phase 4: Integration & Testing (45 mins)

1. Connect frontend to running backend
2. Test all CRUD operations
3. Test error handling
4. Fix bugs and issues

### Phase 5: Polish & Deployment (30 mins)

1. Add loading indicators
2. Improve UI/UX
3. Test responsive design
4. Final testing before submission

## 7. TECHNOLOGY SPECIFICATIONS

### Backend Stack:

- **Framework**: ASP.NET Core 8.0+ (latest)
- **Language**: C#
- **Database**: In-memory List<T>
- **Port**: 5001 (HTTPS)
- **CORS**: Enabled for http://localhost:4200

### Frontend Stack:

- **Framework**: Angular 18+ (latest)
- **Language**: TypeScript
- **Styling**: CSS + Bootstrap 5 (optional but recommended)
- **Port**: 4200 (ng serve default)
- **HTTP Client**: HttpClientModule

## 8. KEY IMPLEMENTATION DETAILS

### Backend Considerations:

- Generate ID automatically (timestamp or counter)
- ISBN uniqueness validation
- Input validation for required fields
- Error responses with meaningful messages
- Enable CORS for Angular app

### Frontend Considerations:

- Two-way data binding for forms
- RxJS Observables for API calls
- Component communication (parent-child)
- Form validation feedback
- Loading states during API calls
- Confirmation dialog before delete
- Toast notifications for success/error

## 9. DEVELOPMENT CHECKLIST

### Backend:

- [ ] Create ASP.NET project
- [ ] Create Book model
- [ ] Create BookService
- [ ] Create BooksController
- [ ] Implement GET all books
- [ ] Implement GET single book
- [ ] Implement POST (create)
- [ ] Implement PUT (update)
- [ ] Implement DELETE
- [ ] Add CORS configuration
- [ ] Test all endpoints

### Frontend:

- [ ] Create Angular project
- [ ] Create Book interface
- [ ] Create BookService
- [ ] Create BookListComponent
- [ ] Create BookFormComponent
- [ ] Create BookItemComponent
- [ ] Implement form validation
- [ ] Add Bootstrap styling
- [ ] Connect to backend API
- [ ] Test all operations
- [ ] Test on different screen sizes

## 10. TESTING SCENARIOS

1. **Add Book**: Submit valid form → Book appears in list
2. **View Books**: Load app → All books displayed
3. **Edit Book**: Click edit → Form populated → Update fields → Save → List updates
4. **Delete Book**: Click delete → Confirmation → Book removed from list
5. **Validation**: Submit empty form → Error messages
6. **Error Handling**: Backend down → User-friendly error message

## 11. SUBMISSION REQUIREMENTS CHECKLIST

- [ ] Complete source code for both projects
- [ ] Backend API working on localhost:5001
- [ ] Frontend working on localhost:4200
- [ ] All CRUD operations functional
- [ ] Screen recording/video of application in action
  - Add a book
  - View all books
  - Edit a book
  - Delete a book
  - Show responsive behavior

## 12. TIME ESTIMATE

- **Total Development Time**: 4-5 hours
- **Testing Time**: 30 minutes
- **Screen Recording & Cleanup**: 30 minutes
- **Total**: 5-6 hours

## 13. RECOMMENDED TOOLS & SETUP

### Required Software:

- Visual Studio 2022 or VS Code
- .NET 8 SDK
- Node.js 18+ & npm
- Angular CLI (`npm install -g @angular/cli`)
- Optional: Postman/Thunder Client for API testing

### Commands for Creation:

```
# Backend
dotnet new webapi -n BookAPI

# Frontend
ng new book-management-frontend
cd book-management-frontend
ng generate component components/book-list
ng generate component components/book-form
ng generate service services/book
```
