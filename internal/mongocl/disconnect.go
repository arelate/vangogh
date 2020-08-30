package mongocl

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
)

func Disconnect(ctx context.Context, cancel context.CancelFunc, client *mongo.Client) {
	cancel()
	client.Disconnect(ctx)
}
