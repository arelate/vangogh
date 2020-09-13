package fetch

import (
	"context"
	"fmt"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {
	fmt.Println(Cmd, args)
	return nil
}
