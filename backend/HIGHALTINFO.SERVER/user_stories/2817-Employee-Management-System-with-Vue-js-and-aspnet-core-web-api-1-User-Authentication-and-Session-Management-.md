# Employee Management System with Vue js and asp.net core web api . 1 . User Authentication and Session Management 

### Secure Login &amp; JWT Authentication

**User Story 1.1.1**  
As a user,  
I want to log in using a secure form with email and password,  
So that I can access the system with proper validation and receive a JWT token for authentication.  
**Acceptance Criteria:**

*   Email must be validated using a standard format.
*   Password must be strong (min 8 chars, uppercase, number, special char).
*   On success, JWT token is issued.

**User Story 1.1.2**  
As a user,  
I want to be automatically logged out after 3 hours of inactivity,  
So that my session expires securely.

**User Story 1.1.3**  
As a system,  
I want to store the username and role in session/local storage,  
So that the user is redirected to their role-specific dashboard.

**User Story 1.1.4**  
As a user,  
I should not be able to navigate back to the login page after logging in using the back button,

So that the session remains secure and navigation is protected.

tested