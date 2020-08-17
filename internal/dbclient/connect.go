package dbclient

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
	"time"
)

func Connect(client *mongo.Client) (context.Context, context.CancelFunc, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	err := client.Connect(ctx)
	return ctx, cancel, err
}
