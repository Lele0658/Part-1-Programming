# **API Endpoint Reference Specification**

System REST API routes, authorization requirements, payload structures, and response specifications.

# **1\. Authentication Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `POST` | `/auth/register` | Register a new user account (Participant by default) | None (Public) | `{ email, password, firstName, lastName, phoneNumber }` | `201 Created` \- user details (without password)`400 Bad Request` \- validation error`409 Conflict` \- email already exists |
| `POST` | `/auth/login` | Authenticate user and return JWT token | None (Public) | `{ email, password }` | `200 OK` \- `{ token, user }401 Unauthorised` \- invalid credentials |

# **2\. User Profile Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `GET` | `/users/me` | Get current logged-in user's profile | Any (logged in) | None | `200 OK` \- user profile (without password)`401 Unauthorised` |
| `PUT` | `/users/me` | Update current user's profile | Any (logged in) | `{ firstName, lastName, phoneNumber }` | `200 OK` \- updated user profile`400 Bad Request` |
| `GET` | `/users/{id}` | Get a specific user's public profile | Any (logged in) | None | `200 OK` \- user profile`404 Not Found` |
| `GET` | `/users` | List all users (admin/organiser view) | Organiser | None | `200 OK` \- list of users |

# **3\. Events Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `GET` | `/events` | List all events (with optional filters: upcoming, past, location) | None (Public) | None | `200 OK` \- list of events |
| `GET` | `/events/{id}` | Get detailed information about a specific event | None (Public) | None | `200 OK` \- event details`404 Not Found` |
| `POST` | `/events` | Create a new event | Organiser | `{ eventName, description, eventDate, location, routeInfo, categories: [{categoryId, maxEntries}] }` | `201 Created` \- event details`400 Bad Request403 Forbidden` |
| `PUT` | `/events/{id}` | Update an existing event | Organiser | `{ eventName, description, eventDate, location, routeInfo }` | `200 OK` \- updated event`404 Not Found403 Forbidden` |
| `DELETE` | `/events/{id}` | Delete/archive an event | Organiser | None | `204 No Content404 Not Found403 Forbidden` |
| `GET` | `/events/{id}/enrolments` | Get all enrolments for a specific event | Organiser | None | `200 OK` \- list of enrolments`404 Not Found` |

# **4\. Categories Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `GET` | `/categories` | List all event categories | None (Public) | None | `200 OK` \- list of categories |
| `GET` | `/categories/{id}` | Get a specific category | None (Public) | None | `200 OK` \- category details`404 Not Found` |
| `POST` | `/categories` | Create a new category | Organiser | `{ categoryName, description, entryFee, minAge, maxAge }` | `201 Created` \- category details`400 Bad Request` |
| `PUT` | `/categories/{id}` | Update a category | Organiser | `{ categoryName, description, entryFee, minAge, maxAge }` | `200 OK` \- updated category`404 Not Found` |
| `DELETE` | `/categories/{id}` | Delete a category | Organiser | None | `204 No Content404 Not Found` |

# **5\. EventCategories Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `POST` | `/events/{eventId}/categories` | Assign a category to an event | Organiser | `{ categoryId, maxEntries }` | `201 Created` \- assignment details`400 Bad Request404 Not Found` |
| `DELETE` | `/events/{eventId}/categories/{categoryId}` | Remove a category from an event | Organiser | None | `204 No Content404 Not Found` |

# **6\. Enrolments Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `POST` | `/events/{eventId}/enrol` | Enrol a participant in an event category | Participant | `{ categoryId }` | `201 Created` \- enrolment details`400 Bad Request` (full, already enrolled)`404 Not Found` |
| `GET` | `/enrolments/me` | Get current user's enrolments | Participant | None | `200 OK` \- list of user's enrolments |
| `GET` | `/enrolments/{id}` | Get a specific enrolment | Participant or Organiser | None | `200 OK` \- enrolment details`404 Not Found403 Forbidden` |
| `DELETE` | `/enrolments/{id}` | Withdraw from an event | Participant | None | `204 No Content404 Not Found403 Forbidden` |
| `PUT` | `/enrolments/{id}/status` | Update enrolment status | Organiser | `{ status }` | `200 OK` \- updated enrolment`404 Not Found` |

# **7\. Results Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `POST` | `/events/{eventId}/results` | Capture results for a participant | Organiser | `{ enrolmentId, timeTaken, position, categoryRank, isCompleted }` | `201 Created` \- result details`400 Bad Request404 Not Found` |
| `PUT` | `/results/{id}` | Update a participant's result | Organiser | `{ timeTaken, position, categoryRank, isCompleted }` | `200 OK` \- updated result`404 Not Found` |
| `GET` | `/results/me` | Get current user's results history | Participant | None | `200 OK` \- list of user's results |
| `GET` | `/events/{eventId}/results` | Get all results for an event | Organiser or Participant | None | `200 OK` \- list of results for the event`404 Not Found` |
| `GET` | `/events/{eventId}/results/leaderboard` | Get leaderboard for an event (by category) | None (Public) | None | `200 OK` \- leaderboard data |

# **8\. Weather Endpoints**

| Method | Route | Description | Role Required | Request Body | Expected Response |
| :---- | :---- | :---- | :---- | :---- | :---- |
| `GET` | `/weather/events/{eventId}` | Get weather forecast for an event | None (Public) | None | `200 OK` \- weather data`404 Not Found` |
| `POST` | `/weather/events/{eventId}` | Add/update weather forecast for an event | Organiser | `{ forecastDate, temperature, precipitation, windSpeed, conditions }` | `201 Created` / `200 OK` \- weather data`400 Bad Request` |

