s3Name=fandango-cc-03-02

aws s3 ls $s3Name  --recursive
aws s3 rm s3://$s3Name --recursive
aws s3api delete-bucket --bucket $s3Name
