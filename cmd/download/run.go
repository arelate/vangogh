package download

import (
	"context"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	if len(args) < 1 {
		args = make([]string, 1)
		args[0] = Cmd
		return help.Run(args)
	}

	fmt.Println(Cmd, args)
	return nil
}
