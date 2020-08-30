package mongocl

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
)

func Connect(client *mongo.Client) (context.Context, context.CancelFunc, error) {
	ctx, cancel := context.WithCancel(context.Background())
	err := client.Connect(ctx)
	return ctx, cancel, err
}
