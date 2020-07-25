from flask import Blueprint
from flask import flash
from flask import g
from flask import redirect
from flask import render_template
from flask import request
from flask import url_for
from werkzeug.exceptions import abort

from flaskapp.auth import login_required
from flaskapp.db import get_db

from flask import jsonify

bp = Blueprint("player", __name__, url_prefix="/player")

@bp.route('/<int:id>/play')
@login_required
def play(id):
    return render_template("player/play.html")
    # return render_template("game/play.html?player={0}".format(id))

@bp.route("/<int:id>/join") #, methods=("GET", "POST"))
@login_required
def join(id):
    db = get_db()
    player = db.execute(
        "SELECT p.id, playernumber, gamename, p.user_id"
        " FROM player p"
        " JOIN game g"
        " ON p.game_id = g.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone()

    if player['user_id'] is not None:
        player = get_player(id, check_user=False)
        abort(403, "Player already occupied by {0}.".format(player['username']))


    user_id = g.user['id']    
    db.execute(
       "UPDATE player"
       " SET user_id = ?"
       " WHERE id = ?",(user_id, id,),
    )

    db.commit()

    player = get_player(id, check_user=False)
    return jsonify(player)  

@login_required
@bp.route("/<int:id>/submit", methods=("GET", "POST"))
def submit(id):
    """Update a post if the current user is the author."""
    db = get_db()
    player = db.execute(
        "SELECT p.id, playernumber, gamename, username, p.user_id, commanddata"
        ", commanddata"
        " FROM player p"
        " JOIN game g" 
        " JOIN user u" 
        " ON p.game_id = g.id AND p.user_id = u.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone() 

    if player['commanddata'] is not None:
        abort(403, "Player {0} {1} has already submitted their turn".format(player['id'], player['username']))

    if request.method == "POST":
        commanddata = request.form["commanddata"]
        error = None

        if not commanddata:
            error = "commanddata is required."

        if error is not None:
            flash(error)
        else:
            db = get_db()
            db.execute(
                "UPDATE player SET commanddata = ? WHERE id = ?", (commanddata, id)
            )
            db.commit()
            return "Submitted orders for "+player['username']
            # return redirect(url_for("blog.index"))

    # return jsonify(player)

    return render_template("game/submit.html", player=player)  

def get_player(id, check_user=True):
    """Get a post and its author by id.

    Checks that the id exists and optionally that the current user is
    the author.

    :param id: id of post to get
    :param check_user: require the current user to be the author
    :return: the post with author information
    :raise 404: if a post with the given id doesn't exist
    :raise 403: if the current user isn't the author
    """
    db = get_db()
    player = db.execute(
        "SELECT p.id, playernumber, gamename, username, p.user_id"
        ", commanddata"
        " FROM player p"
        " JOIN game g" 
        " JOIN user u" 
        " ON p.game_id = g.id AND p.user_id = u.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone()   

    if player is None:
        abort(404, "Player id {0} doesn't exist.".format(id))

    if check_user and player["user_id"] != g.user["id"]:
        abort(403)

    return player