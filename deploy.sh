
cp -r landingPage/* covideogame/;
cp Assets/Resources/favicon.png covideogame/;
aws s3 sync ~/Desktop/covideogame/covideogame s3://covideogame/;