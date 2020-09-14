package fetch

import (
	"context"
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/gog/const/aliases"
	"github.com/boggydigital/vangogh/internal/gog/const/cmds"
	"github.com/boggydigital/vangogh/internal/gog/const/names"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/remote"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	ok, err := help.Intercept(cmds.Fetch, args[1:])
	if ok || err != nil {
		return err
	}

	mt, err := media.ParseArgs(cmds.Fetch, args[1:])
	if err != nil {
		return err
	}

	var transferor remote.Transferor
	var setter local.Setter

	switch args[0] {
	case aliases.Details:
		fallthrough
	case names.Details:
		transferor = remote.NewDetails(httpClient, urls.Details(mt))
		setter = local.NewDetails(mongoClient, ctx)
	default:
		return errors.New(fmt.Sprintf("unknown %s source: %s", cmds.Fetch, args[0]))
	}

	if transferor == nil ||
		setter == nil {
		return errors.New("error creating source or destination for " + cmds.Fetch)
	}

	err = transferor.Transfer(1, setter)
	if err != nil {
		return err
	}

	fmt.Println(cmds.Fetch, args)
	return nil
}
