-- Initialize the database.
-- Drop any existing data and create empty tables.

DROP TABLE IF EXISTS user;
DROP TABLE IF EXISTS game;
DROP TABLE IF EXISTS player;

CREATE TABLE user (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT UNIQUE NOT NULL,
  created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  password TEXT NOT NULL
);

CREATE TABLE game (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  summary TEXT NOT NULL,
  gamename TEXT NOT NULL,
  numplayers INTEGER NOT NULL,
  turnnum INTEGER,
  body TEXT
);

CREATE TABLE player (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  game_id INTEGER NOT NULL,
  user_id INTEGER,
  slot INTEGER,
  FOREIGN KEY (user_id) REFERENCES user (id),
  FOREIGN KEY (game_id) REFERENCES game (id)
);
