package changes

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
)

var mongoClient *mongo.Client
var ctx context.Context

func Init(client *mongo.Client, contxt context.Context) {
	mongoClient = client
	ctx = contxt

	if mongoClient == nil ||
		ctx == nil {
		panic("cannot initialize changes with no database access")
	}
}
