import sqlite3

import click
from flask import current_app
from flask import g
from flask.cli import with_appcontext

def dict_factory(cursor, row):
    d = {}
    for idx, col in enumerate(cursor.description):
        d[col[0]] = row[idx]
    return d

def get_db():
    """Connect to the application's configured database. The connection
    is unique for each request and will be reused if this is called
    again.
    """
    if "db" not in g:
        g.db = sqlite3.connect(
            current_app.config["DATABASE"], detect_types=sqlite3.PARSE_DECLTYPES
        )
        # g.db.row_factory = sqlite3.Row
        g.db.row_factory = dict_factory

    return g.db


def close_db(e=None):
    """If this request connected to the database, close the
    connection.
    """
    db = g.pop("db", None)

    if db is not None:
        db.close()


def init_db():
    """Clear existing data and create new tables."""
    db = get_db()

    with current_app.open_resource("schema.sql") as f:
        db.executescript(f.read().decode("utf8"))

    from werkzeug.security import generate_password_hash

    user_ids = {}
    for username in ('tim','mark','matt','stephan'):
        db.execute(
            "INSERT INTO user (username, password) VALUES (?, ?)",
            (username, generate_password_hash('test')),
        )
        db.commit()

        user_ids[username] = (
            db.execute(
                "SELECT id "
                " FROM user "
                " WHERE username = ?",
            (username,),
            )
            .fetchone()
        )['id']

    game_ids = {}
    for gamename in ('game01','game02'):
        db.execute(
            "INSERT INTO game (gamename,summary,numplayers) VALUES (?,?,?)",
            (gamename,"a summary", 3 ),
        )
        db.commit()

        game = (
            db.execute(
                "SELECT id, numplayers"
                " FROM game"
                " WHERE gamename = ?",
            (gamename,),
            )
            .fetchone()
        )

        game_ids[gamename] = game['id']

        for i in range(0,game['numplayers']):
        # for i in range(0,3):
            db.execute(
                "INSERT INTO player (game_id,slot) VALUES (?,?)",
                (game['id'], str(i)),
            )
        db.commit()            


    # for game,usernames in [
    #     ('game01', ('tim','mark')),
    #     ('game02', ('tim','matt')),
    # ]:

    #     slot = 0
    #     for username in usernames:
    #         user_id = user_ids[username]
    #         db.execute(
    #             "INSERT INTO player (user_id,game_id,slot) VALUES (?,?,?)",
    #             (user_id, game_ids[game], slot),
    #         )
    #         slot += 1
    #         db.commit()




@click.command("init-db")
@with_appcontext
def init_db_command():
    """Clear existing data and create new tables."""
    init_db()
    click.echo("Initialized the database.")

def init_app(app):
    """Register database functions with the Flask app. This is called by
    the application factory.
    """
    app.teardown_appcontext(close_db)
    app.cli.add_command(init_db_command)
