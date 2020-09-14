package fetchall

import (
	"context"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/gog/const/cmds"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/remote"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func printDot(index int) {
	fmt.Print(".")
}

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	ok, err := help.Intercept(cmds.FetchAll, args)
	if ok || err != nil {
		return err
	}

	mt, err := media.ParseArgs(cmds.FetchAll, args[1:])
	if err != nil {
		return err
	}

	pageTransferor, err := remote.GetPageTransferorByName(args[0], mt, httpClient)
	if err != nil {
		return err
	}

	setter, err := local.GetSetterByName(args[0], mongoClient, ctx)
	if err != nil {
		return err
	}

	totalPages := 1
	for page := 1; page <= totalPages; page++ {
		fmt.Println(cmds.FetchAll, args[0], fmt.Sprintf("(%s)", mt.String()), page, "of", totalPages)
		totalPages, err = pageTransferor.TransferPage(page, setter, printDot)
		fmt.Println()
		if err != nil {
			return err
		}
	}

	return nil
}
