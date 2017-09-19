USE [IzendaCourseManagementSystem]

CREATE TABLE Instructor
(
	Id int PRIMARY KEY NOT NULL,
	FirstName varchar(50) NULL,
	LastName varchar(50) NULL,
	HireDate DateTime NULL,
	UserName varchar(50) NOT NULL,
	Password varchar(50) NOT NULL,
	UserType varchar(15) NOT NULL
)

CREATE TABLE Student
(
	Id int PRIMARY KEY NOT NULL,
	FirstName varchar(50) NULL,
	LastName varchar(50) NULL,
	GPA float NULL,
	UserName varchar(50) NOT NULL,
	Password varchar(50) NOT NULL,
	CreditHours int NULL,
	UserType varchar(15) NOT NULL
)

CREATE TABLE Administrator
(
	Id int PRIMARY KEY NOT NULL,
	FirstName varchar(50) NULL,
	LastName varchar(50) NULL,
	HireDate DateTime NULL,
	UserName varchar(50) NOT NULL,
	Password varchar(50) NOT NULL,
	UserType varchar(15) NOT NULL
)

CREATE TABLE Course
(
	Id int PRIMARY KEY NOT NULL,
	StartDate DateTime NULL,
	EndDate DateTime NULL,
	CreditHours int NOT NULL,
	CourseName varchar(50) NOT NULL,
	CourseDescription varchar(100) NULL
)

CREATE TABLE CourseGrades
(
	Id int PRIMARY KEY NOT NULL,
	CourseId int FOREIGN KEY REFERENCES Course(Id) NOT NULL,
	FinalGrade char NULL
)

CREATE TABLE Instructor_Course
(
	Id int PRIMARY KEY NOT NULL,
	InstructorId int FOREIGN KEY REFERENCES Instructor(Id) NOT NULL,
	CourseId int FOREIGN KEY REFERENCES Course(Id) NOT NULL
)

CREATE TABLE Student_CourseGrades
(
	Id int PRIMARY KEY NOT NULL,
	StudentId int FOREIGN KEY REFERENCES Student(Id) NOT NULL,
	CourseGradesId int FOREIGN KEY REFERENCES CourseGrades(Id) NOT NULL
)

CREATE TABLE Student_Course
(
	Id int PRIMARY KEY NOT NULL,
	StudentId int FOREIGN KEY REFERENCES Student(Id) NOT NULL,
	CourseId int FOREIGN KEY REFERENCES Course(Id) NOT NULL
)

INSERT INTO Administrator (Id, FirstName, LastName, HireDate, UserName, Password, UserType)
VALUES (1, 'Admin', 'One', '2017-09-09', 'admin1', 'AdminOne', 'Administrator');

INSERT INTO Instructor (Id, FirstName, LastName, HireDate, UserName, Password, UserType)
VALUES (100, 'John', 'Doe', '2017-09-10', 'johndoe', 'IAmJohnDoe', 'Instructor');
INSERT INTO Instructor (Id, FirstName, LastName, HireDate, UserName, Password, UserType)
VALUES (101, 'Mary', 'Moe', '2017-09-10', 'marymoe', 'IAmMaryMoe', 'Instructor');
INSERT INTO Instructor (Id, FirstName, LastName, HireDate, UserName, Password, UserType)
VALUES (102, 'David', 'Throe', '2017-09-10', 'davidthroe', 'IAmDavidThroe', 'Instructor');

INSERT INTO Student (Id, FirstName, LastName, GPA, UserName, Password, CreditHours, UserType)
VALUES (5000, 'Larry', 'Low', 3.3, 'larrylow', 'TheRealLarryLow', 56, 'Student');
INSERT INTO Student (Id, FirstName, LastName, GPA, UserName, Password, CreditHours, UserType)
VALUES (5001, 'Natalie', 'Ngo', 4.0, 'nataliengo', 'TheRealNatalieNgo', 104, 'Student');
INSERT INTO Student (Id, FirstName, LastName, GPA, UserName, Password, CreditHours, UserType)
VALUES (5002, 'Mark', 'Llark', 1.7, 'markllark', 'TheRealMarkLlark', 3, 'Student');

INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10001302, '2017-08-14', '2017-12-05', 4, 'CSCI1302', 'Software Development');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10001730, '2017-08-14', '2017-12-05', 4, 'CSCI1730', 'Systems Programming');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10002670, '2017-08-14', '2017-12-05', 4, 'CSCI2670', 'Intro to the Theory of Computation');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10002720, '2017-08-14', '2017-12-05', 4, 'CSCI2720', 'Data Structures');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10003030, '2017-08-14', '2017-12-05', 3, 'CSCI3030', 'Computer Ethics');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10014600, '2017-08-14', '2017-12-05', 3, 'MIST4600', 'Computer Programming in Business');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10014620, '2017-08-14', '2017-12-05', 3, 'MIST4620', 'Systems Analysis and Design');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10015620, '2017-08-14', '2017-12-05', 3, 'MIST5620', 'Business Intelligence');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10022250, '2017-08-14', '2017-12-05', 4, 'MATH2250', 'Calculus I for Science and Engineering');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10022260, '2017-08-14', '2017-12-05', 4, 'MATH2260', 'Calculus II for Science and Engineering');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10022270, '2017-08-14', '2017-12-05', 4, 'MATH2270', 'Calculus III for Science and Engineering');
INSERT INTO Course (Id, StartDate, EndDate, CreditHours, CourseName, CourseDescription)
VALUES (10023300, '2017-08-14', '2017-12-05', 3, 'MATH3300', 'Applied Linear Algebra');

INSERT INTO CourseGrades (Id, CourseId, FinalGrade) VALUES (1, 10001302, 'B');
INSERT INTO CourseGrades (Id, CourseId, FinalGrade) VALUES (2, 10001302, 'A');
INSERT INTO CourseGrades (Id, CourseId, FinalGrade) VALUES (3, 10001730, 'A');
INSERT INTO CourseGrades (Id, CourseId, FinalGrade) VALUES (4, 10002670, 'A');

INSERT INTO Student_CourseGrades (Id, StudentId, CourseGradesId) VALUES (1, 5000, 1);
INSERT INTO Student_CourseGrades (Id, StudentId, CourseGradesId) VALUES (2, 5001, 2);
INSERT INTO Student_CourseGrades (Id, StudentId, CourseGradesId) VALUES (3, 5001, 3);
INSERT INTO Student_CourseGrades (Id, StudentId, CourseGradesId) VALUES (4, 5001, 4);

INSERT INTO Student_Course (Id, StudentId, CourseId) VALUES (1, 5000, 10001730);