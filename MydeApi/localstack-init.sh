#!/bin/bash

echo "Criando fila SQS..."
awslocal sqs create-queue --queue-name proposals-queue

echo "Fila criada com sucesso!"
