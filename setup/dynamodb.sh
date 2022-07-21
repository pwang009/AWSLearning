tableName=ProductCatalog
aws dynamodb create-table --table-name $tableName \
    --attribute-definitions AttributeName=Id,AttributeType=N --key-schema AttributeName=Id,KeyType=HASH \
    --provisioned-throughput ReadCapacityUnits=1,WriteCapacityUnits=1 --region us-west-2 \
    --query TableDescription.TableArn --output text

aws dynamodb describe-table --table-name $tableName | grep TableStatus
aws dynamodb batch-write-item --request-items file://$tableName.json
aws dynamodb delete-table --table-name $tableName

tableName=WeatherForecast
aws dynamodb create-table --table-name $tableName \
    --attribute-definitions AttributeName=City,AttributeType=S AttributeName=Date,AttributeType=S \
    --key-schema AttributeName=City,KeyType=HASH AttributeName=Date,KeyType=HASH \
    --provisioned-throughput ReadCapacityUnits=1,WriteCapacityUnits=1 --region us-west-2 \
    --query TableDescription.TableArn --output text
