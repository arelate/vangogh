// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package fetch

import (
	"context"
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/remote"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/strings/aliases"
	"github.com/boggydigital/vangogh/internal/strings/cmds"
	"github.com/boggydigital/vangogh/internal/strings/names"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
	"strconv"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	ok, err := help.Intercept(cmds.Fetch, args[1:])
	if ok || err != nil {
		return err
	}

	mt, targs, err := media.ParseArgs(cmds.Fetch, args[1:])
	if err != nil {
		return err
	}

	ids := make([]int, 0)
	invalidIds := make([]string, 0)
	for _, arg := range targs {
		id, err := strconv.Atoi(arg)
		if err != nil {
			invalidIds = append(invalidIds, arg)
		} else {
			ids = append(ids, id)
		}
	}
	if len(invalidIds) > 0 {
		fmt.Println("NOTE: Invalid ids format or value: ", invalidIds)
	}

	var transferor remote.Transferor
	var setter local.Setter

	scope := args[0]

	switch scope {
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

	for i, id := range ids {
		err = transferor.Transfer(id, setter)
		if err != nil {
			return err
		}

		fmt.Println(cmds.Fetch, names.Full(scope), "id =", id, ";", i+1, "of", len(ids))
	}

	return nil
}
