package cmd

import (
	"context"
	"github.com/boggydigital/vangogh/cmd/download"
	"github.com/boggydigital/vangogh/cmd/fetch"
	"github.com/boggydigital/vangogh/cmd/fetchall"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/cmd/version"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	// Show general help if no arguments have been provided
	if len(args) < 2 {
		args = make([]string, 2)
		args[1] = help.Cmd
	}

	switch args[1] {
	// version
	case version.Alias:
		fallthrough
	case version.Cmd:
		return version.Run()
	// help
	case help.Alias:
		fallthrough
	case help.Cmd:
		return help.Run(args[2:])
	// fetch
	case fetch.Alias:
		fallthrough
	case fetch.Cmd:
		return fetch.Run(httpClient, mongoClient, ctx, args[2:])
	// fetch all
	case fetchall.Alias:
		fallthrough
	case fetchall.Cmd:
		return fetchall.Run(httpClient, mongoClient, ctx, args[2:])
	// download
	case download.Alias:
		fallthrough
	case download.Cmd:
		return download.Run(httpClient, mongoClient, ctx, args[2:])
	// unknown
	default:
		return help.Run(args[2:])
	}
}
