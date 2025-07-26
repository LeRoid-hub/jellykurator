-- https://dbdiagram.io/d/jellykurator-688524cfcca18e685cdf930b

CREATE TABLE "movies" (
  "id" integer PRIMARY KEY,
  "name" varchar,
  "originaltitle" varchar,
  "imdb" varchar,
  "databases" varchar
);

CREATE TABLE "tvshows" (
  "id" integer PRIMARY KEY,
  "name" varchar,
  "originaltitle" varchar,
  "imdb" varchar
);

CREATE TABLE "seasons" (
  "id" integer PRIMARY KEY,
  "tvshow" integer NOT NULL,
  "name" varchar,
  "seasonnumber" integer
);

CREATE TABLE "episodes" (
  "id" integer PRIMARY KEY,
  "season" integer NOT NULL,
  "name" varchar,
  "episodenumber" integer
);

CREATE TABLE "users" (
  "id" integer PRIMARY KEY,
  "name" varchar,
  "password" varchar,
  "email" varchar,
  "token" varchar
);

CREATE TABLE "owns" (
  "content_id" integer NOT NULL,
  "user_id" integer NOT NULL
);

ALTER TABLE "owns" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");

CREATE TABLE "movies_owns" (
  "movies_id" integer,
  "owns_content_id" integer,
  PRIMARY KEY ("movies_id", "owns_content_id")
);

ALTER TABLE "movies_owns" ADD FOREIGN KEY ("movies_id") REFERENCES "movies" ("id");

ALTER TABLE "movies_owns" ADD FOREIGN KEY ("owns_content_id") REFERENCES "owns" ("content_id");


CREATE TABLE "episodes_owns" (
  "episodes_id" integer,
  "owns_content_id" integer,
  PRIMARY KEY ("episodes_id", "owns_content_id")
);

ALTER TABLE "episodes_owns" ADD FOREIGN KEY ("episodes_id") REFERENCES "episodes" ("id");

ALTER TABLE "episodes_owns" ADD FOREIGN KEY ("owns_content_id") REFERENCES "owns" ("content_id");


ALTER TABLE "episodes" ADD FOREIGN KEY ("id") REFERENCES "seasons" ("id");

ALTER TABLE "seasons" ADD FOREIGN KEY ("id") REFERENCES "tvshows" ("id");
