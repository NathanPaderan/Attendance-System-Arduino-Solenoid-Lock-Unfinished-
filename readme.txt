install Wamp
Create database named "registration"
Just click the database name "registration" then click the "SQL" at the top
Query eto pto create table . . . copy paste to "SQL query"

CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `username` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;