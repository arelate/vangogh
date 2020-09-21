// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package download

import (
	"context"
	"flag"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/strings/cmds"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	if len(args) < 1 {
		args = make([]string, 1)
		args[0] = cmds.Download
		return help.Run(args)
	}

	downloadFlags := flag.NewFlagSet(cmds.Download, flag.ExitOnError)
	osFlag := downloadFlags.String("os", "", "comma-separated list of operating systems, no spaces: windows, linux, macos")
	langFlag := downloadFlags.String("lang", "", "comma-separated list of language codes, no spaces")

	err := downloadFlags.Parse(args)
	if err != nil {
		return err
	}

	fmt.Println(cmds.Download, *osFlag, *langFlag)
	return nil
}
