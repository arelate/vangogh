// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package fetchall

import (
	"context"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/remote"
	"github.com/boggydigital/vangogh/internal/strings/cmds"
	"github.com/boggydigital/vangogh/internal/strings/names"
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

	mt, targs, err := media.ParseArgs(cmds.FetchAll, args[1:])
	if err != nil {
		return err
	}

	if len(targs) > 0 {
		fmt.Println("NOTE: Ignored options: ", targs)
	}

	scope := args[0]

	pageTransferor, err := remote.GetPageTransferorByName(scope, mt, httpClient)
	if err != nil {
		return err
	}

	setter, err := local.GetSetterByName(scope, mongoClient, ctx)
	if err != nil {
		return err
	}

	totalPages := 1
	for page := 1; page <= totalPages; page++ {
		fmt.Println(cmds.FetchAll, names.Full(scope), fmt.Sprintf("(%s)", mt.String()), page, "of", totalPages)
		totalPages, err = pageTransferor.TransferPage(page, setter, printDot)
		fmt.Println()
		if err != nil {
			return err
		}
	}

	return nil
}
