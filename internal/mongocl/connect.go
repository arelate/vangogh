package mongocl

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
)

func Connect(client *mongo.Client) (context.Context, context.CancelFunc, error) {
	ctx, cancel := context.WithCancel(context.Background())
	err := client.Connect(ctx)
	if err != nil {
		return ctx, cancel, err
	}
	err = client.Ping(ctx, nil)
	if err != nil {
		return ctx, cancel, err
	}
	return ctx, cancel, err
}
