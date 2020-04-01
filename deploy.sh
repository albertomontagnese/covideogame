
cp index.html covideogame/index.html;
cp Assets/Resources/favicon.png covideogame/;
aws s3 sync ~/Desktop/covideogame/covideogame s3://covideogame/;