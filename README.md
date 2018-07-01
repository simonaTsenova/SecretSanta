# SecretSanta

## Summary
This is a Web API server side project that helps people exchanging christmas gifts. It automates the typical process of the so called Secret Santa "game" practised in many organisations, schools, etc. After creating an account a user can create groups and add other people to them by sending invitations. Each user can decide to accept or delete invitation for a certain group. After a group has reached enough members, the admin can start the linking process. In this process each user gets a person from the group to give present to.

## Description
### Structure
The architecture consists of the following layers:
* Models - contains all the entities used to create the database
* Data - contains the database context, repositories and unit of work used by EntityFramework to communicate with the database
* Services - contains methods to call the data layer to add, retrieve, update or delete data from database
* Factories - contains factories contracts to create models
* Authentication - contains authentication abstractions
* Providers - contains abstractions of classes (such as HTTPContext)
* Common - contains common stuff to be used everywhere like custom exceptions, filters and constants
* Web - contains controllers (i.e. logic for all the endpoints provided by the project) and main project setups

### Endpoints
The project exposes the following endpoints:
* **POST api/users*** - can be used to create a new user (register)
* **POST api/login*** - can be used to get access token for authorized endpoints
* **GET api/users?skip={s}&take={t}&order={Asc|Desc}&search={phrase}*** - returns all users; results may be filtered by phrase, ordered (ascending or descending) and paginated if respective query parameters are provided
* **GET api/users/{username}*** - returns user with provided username
* **GET api/users/{username}/groups?skip={s}&take={t}*** - returns groups of user with provided username; results may be paginated if skip and take parameters are provided
* **POST api/users/{username}/invitations*** - can be used to send invitation to a user with provided username; the body consists of groupName and sentDate; only admins can invite members to groups
* **GET api/users/{username}/invitations?skip={s}&take={t}&order={A|D}*** - returns invitations of user with provided username; results may be ordered (ascending or descending) and paginated if query parameters are provided
* **GET api/users/{username}/groups/{groupname}/links*** - return link (member of group to give present to) of user with provided username for group with provided groupname

* **POST api/groups*** - can be used to create a new group; the body must consist of name for the new group
* **GET api/groups/{groupname}/participants*** - returns all participants of a group with provided groupname
* **DELETE api/groups/{groupname}/participants/{participantUsername}*** - can be used to delete member with provided participantUsername from group with provided groupName; only admins can remove participants from group
* **POST api/groups/{groupname}/invitations*** - can be used to accept invitation if current user has one for group with provided groupname
* **DELETE api/users/{username}/invitations/{id}*** - can be used to delete invitation with provided id of user with provided username
* **POST api/groups/{groupname}/links*** - starts linking process for a group with provided groupname; only admins can start linking process for a group

*endpoints are authorized

### Unit testing
Project contains tests for Services layer and Controllers from Web layer.
