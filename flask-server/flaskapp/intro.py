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


bp = Blueprint("into", __name__)


@bp.route("/")
def index():
    return {"id":997, "name" : "Flo"}

