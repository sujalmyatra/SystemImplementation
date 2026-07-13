## Student Course Enrollment System

The Student Course Enrollment System is designed to manage students, courses, enrollments, teachers, attendance, and marks. The system allows students to enroll in multiple courses, and each course can have multiple students.

### Main Entities

The system will have the following main entities:

1. **Student**
   A student represents a learner who can enroll in different courses.

2. **Course**
   A course represents a subject or training program offered by the institute.

3. **Enrollment**
   Enrollment represents the relationship between student and course.

4. **Teacher**
   A teacher represents a person who teaches one or more courses.

5. **Attendance**
   Attendance represents student presence or absence for a course session.

6. **Marks**
   Marks represent the score obtained by a student in a course.

---

### Relationships

1. **Student to Course**
   One student can enroll in multiple courses.

2. **Course to Student**
   One course can have multiple students.

This is a **many-to-many relationship**.

Because many-to-many needs extra information like enrollment date and status, we use an intermediate entity:

**Student → Enrollment → Course**

Example:

Student A enrolled in:

* C# Course
* SQL Course
* ASP.NET Core Course

C# Course has:

* Student A
* Student B
* Student C

---

3. **Teacher to Course**
   One teacher can teach multiple courses.

4. **Enrollment to Attendance**
   One enrollment can have multiple attendance records.

5. **Enrollment to Marks**
   One enrollment can have one marks record for a course.

---

### Enrollment Status

The system should support the following enrollment statuses:

1. **Active**
   Student is currently enrolled in the course.

2. **Completed**
   Student has completed the course.

3. **Cancelled**
   Student cancelled the enrollment.

4. **Failed**
   Student failed the course.

---

### Enrollment Rules

The system should allow students to enroll in courses.

1. A student can enroll in multiple courses.

2. A course can have multiple students.

3. Student should not be enrolled in the same course twice with active status.

4. Student cannot enroll in an inactive course.

5. Course should not exceed maximum student capacity.

Example:

If C# Course has capacity of 30 students and already has 30 active enrollments, new enrollment should not be allowed.

---

### Attendance Rules

The system should maintain attendance for students.

1. Attendance should be marked course-wise.

2. Attendance can be marked only for active enrollments.

3. Attendance status can be:

* Present
* Absent

4. Attendance date should not be in the future.

---

### Marks Rules

The system should maintain marks for each enrolled student.

1. Marks can be added only for active or completed enrollment.

2. Marks should not be less than 0.

3. Marks should not be greater than 100.

4. If marks are 35 or above, student should be marked as Passed.

5. If marks are less than 35, student should be marked as Failed.

---

### Business Rules

1. Duplicate active enrollment for the same student and course should not be allowed.

2. Course capacity should not be exceeded.

3. Inactive courses should not allow new enrollments.

4. Attendance should be recorded only for enrolled students.

5. Marks should be recorded only for enrolled students.

6. Enrollment history should not be permanently deleted.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Student
* Course
* Enrollment
* Attendance
* Marks

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve student enrollment history, attendance history, and marks history.

---

### Reports

The system should provide the following reports:

#### 1. Course-wise Student Count Report

Show how many students are enrolled in each course.

Example:

* C# Course: 30 students
* SQL Course: 25 students

---

#### 2. Student Course History Report

Show all courses enrolled by a selected student.

Example:

Student A:

* C# Course
* SQL Course
* ASP.NET Core Course

---

#### 3. Teacher-wise Course Report

Show courses handled by each teacher.

Example:

* Teacher A: C#, ASP.NET Core
* Teacher B: SQL, EF Core

---

#### 4. Attendance Report

Show attendance percentage of each student for a course.

Example:

Student A:

* Present: 18 days
* Total Sessions: 20 days
* Attendance: 90%

---

#### 5. Failed Student Report

Show students who failed in any course.

Example:

* Student A failed in SQL
* Student B failed in C#

---

#### 6. Course Capacity Report

Show courses that are full or almost full.

Example:

* C# Course: 30/30 students
* SQL Course: 28/30 students

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show course-wise active student count.

2. Show student-wise enrolled course count.

3. Show students enrolled in a selected course.

4. Show courses in which a selected student is enrolled.

5. Show courses with no students.

6. Show students who enrolled in more than 3 courses.

7. Show attendance percentage of each student for a course.

8. Show students who failed in any course.

9. Show teacher-wise course count.

10. Show courses where capacity is full.

11. Show students who have no attendance records.

12. Show top 5 students by marks in a selected course.

---

### Overall System Goal

The main goal of this system is to manage students, courses, enrollments, attendance, marks, and reports. The system should correctly handle the many-to-many relationship between students and courses, prevent duplicate enrollments, maintain course capacity, track attendance, store marks, and generate useful academic reports.
