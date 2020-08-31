package cli

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/cli/fetch"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
	"os"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, args []string) error {

	if len(args) < 2 {
		// help
		fmt.Println("vangogh help - not enough args")
		os.Exit(1)
	}

	switch os.Args[1] {
	case fetch.Cmd:
		return fetch.Run(httpClient, mongoClient, args)
	default:
		fmt.Println("vangogh help - unknown cmd")
	}

	return nil
}
