package cmd

import (
	"context"
	"github.com/boggydigital/vangogh/cmd/download"
	"github.com/boggydigital/vangogh/cmd/fetch"
	"github.com/boggydigital/vangogh/cmd/fetchall"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/cmd/version"
	"github.com/boggydigital/vangogh/internal/gog/const/aliases"
	"github.com/boggydigital/vangogh/internal/gog/const/cmds"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	ok, err := help.Intercept("", args[1:])
	if ok || err != nil {
		return err
	}

	switch args[1] {
	// version
	case aliases.Version:
		fallthrough
	case cmds.Version:
		return version.Run()
	// help
	case aliases.Help:
		fallthrough
	case cmds.Help:
		return help.Run(args[2:])
	// fetch
	case aliases.Fetch:
		fallthrough
	case cmds.Fetch:
		return fetch.Run(httpClient, mongoClient, ctx, args[2:])
	// fetch all
	case aliases.FetchAll:
		fallthrough
	case cmds.FetchAll:
		return fetchall.Run(httpClient, mongoClient, ctx, args[2:])
	// download
	case aliases.Download:
		fallthrough
	case cmds.Download:
		return download.Run(httpClient, mongoClient, ctx, args[2:])
	// unknown
	default:
		return help.Run(args[2:])
	}
}
