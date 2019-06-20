#!/bin/sh

git filter-branch --env-filter '

OLD_EMAIL="KennyHong"
CORRECT_NAME="Kenny Hong"
CORRECT_EMAIL="k.hongjiahan@gmail.com"

if [ "$GIT_COMMITTER_NAME" = "$OLD_EMAIL" ]
then
    export GIT_COMMITTER_NAME="$CORRECT_NAME"
    export GIT_COMMITTER_EMAIL="$CORRECT_EMAIL"
fi
if [ "$GIT_AUTHOR_NAME" = "$OLD_EMAIL" ]
then
    export GIT_AUTHOR_NAME="$CORRECT_NAME"
    export GIT_AUTHOR_EMAIL="$CORRECT_EMAIL"
fi
' --tag-name-filter cat -- --branches --tags