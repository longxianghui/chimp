
-- ----------------------------
-- Table structure for school
-- ----------------------------
DROP TABLE IF EXISTS School;
CREATE TABLE School (
    Id   CHAR(32)     PRIMARY KEY
                       NOT NULL,
    Name VARCHAR (255) 
);

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS Student;
CREATE TABLE Student (
    Id       CHAR(32)     NOT NULL
                           PRIMARY KEY,
    Name     VARCHAR (255),
    Age      INT,
    Birthday DATETIME,
    SchoolId CHAR (32) 
); 
