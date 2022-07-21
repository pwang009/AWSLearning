appName=bsLearning
aws elasticbeanstalk check-dns-availability --cname-prefix $appName
aws elasticbeanstalk create-application --application-name $appName --description "Learning Beanstalk"
aws elasticbeanstalk describe-application-versions --application-name $appName --version-label v1

cd api 
dotnet publish -o site
zip -r site.zip site
aws s3 cp site.zip s3://wygroup-lambda-bucket

aws elasticbeanstalk create-application-version --application-name $appName \
    --version-label v1 --source-bundle S3Bucket=wygroup-lambda-bucket,S3Key=site.zip