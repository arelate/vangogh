// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"os"
	"path/filepath"

	"github.com/arelate/vangogh_cli_api/cli"
	"github.com/arelate/vangogh_cli_api/cli/dirs"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/app"
	"github.com/boggydigital/vangogh/clo_delegates"
	"github.com/boggydigital/wits"
)

var (
	//go:embed "cli-commands.txt"
	cliCommands []byte
	//go:embed "cli-help.txt"
	cliHelp []byte
)

const (
	settingsFilename    = "settings.txt"
	directoriesFilename = "directories.txt"
)

var (
	configDir = "/etc/vangogh"
	logsDir   = "/var/log/vangogh"
	stateDir  = "/var/lib/vangogh"
	tempDir   = "/var/tmp"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	if err := readUserDirectories(); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	//set directories context in vangogh_cli_api
	dirs.SetTempDir(tempDir)
	dirs.SetStateDir(stateDir)

	defs, err := clo.Load(
		bytes.NewBuffer(cliCommands),
		bytes.NewBuffer(cliHelp),
		clo_delegates.Values)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	userDefaultsPath := filepath.Join(configDir, settingsFilename)
	if _, err := os.Stat(userDefaultsPath); err == nil {
		udoFile, err := os.Open(userDefaultsPath)
		if err != nil {
			_ = ns.EndWithError(err)
			os.Exit(1)
		}
		userDefaultsOverrides, err := wits.ReadKeyValues(udoFile)
		if err != nil {
			_ = ns.EndWithError(err)
			os.Exit(1)
		}
		if err := defs.SetUserDefaults(userDefaultsOverrides); err != nil {
			_ = ns.EndWithError(err)
			os.Exit(1)
		}
	}

	if defs.HasUserDefaultsFlag("debug") {
		logger, err := nod.EnableFileLogger(logsDir)
		if err != nil {
			_ = ns.EndWithError(err)
			os.Exit(1)
		}
		defer logger.Close()
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"auth":             cli.AuthHandler,
		"cleanup":          cli.CleanupHandler,
		"dehydrate-images": cli.DehydrateImagesHandler,
		"digest":           cli.DigestHandler,
		"export":           cli.ExportHandler,
		"get-data":         cli.GetDataHandler,
		"get-downloads":    cli.GetDownloadsHandler,
		"get-images":       cli.GetImagesHandler,
		"get-items":        cli.GetItemsHandler,
		"get-summary":      cli.GetSummaryHandler,
		"get-thumbnails":   cli.GetThumbnailsHandler,
		"get-videos":       cli.GetVideosHandler,
		"info":             cli.InfoHandler,
		"list":             cli.ListHandler,
		"local-tag":        cli.LocalTagHandler,
		"owned":            cli.OwnedHandler,
		"reduce":           cli.ReduceHandler,
		"search":           cli.SearchHandler,
		"serve":            cli.ServeHandler,
		"size":             cli.SizeHandler,
		"summarize":        cli.SummarizeHandler,
		"sync":             cli.SyncHandler,
		"tag":              cli.TagHandler,
		"update-downloads": cli.UpdateDownloadsHandler,
		"validate":         cli.ValidateHandler,
		"version":          app.VersionHandler,
		"vet":              cli.VetHandler,
		"wishlist":         cli.WishlistHandler,
	})

	if err := defs.AssertCommandsHaveHandlers(); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}
}

func readUserDirectories() error {
	if _, err := os.Stat(directoriesFilename); os.IsNotExist(err) {
		return nil
	}

	udFile, err := os.Open(directoriesFilename)
	if err != nil {
		return err
	}

	dirs, err := wits.ReadKeyValue(udFile)
	if err != nil {
		return err
	}

	if cd, ok := dirs["config"]; ok {
		configDir = cd
	}
	if ld, ok := dirs["logs"]; ok {
		logsDir = ld
	}
	if sd, ok := dirs["state"]; ok {
		stateDir = sd
	}
	if td, ok := dirs["temp"]; ok {
		tempDir = td
	}

	//validate that directories actually exist
	if _, err := os.Stat(configDir); err != nil {
		return err
	}

	if _, err := os.Stat(logsDir); err != nil {
		return err
	}

	if _, err := os.Stat(stateDir); err != nil {
		return err
	}

	if _, err := os.Stat(tempDir); err != nil {
		return err
	}

	return nil
}
