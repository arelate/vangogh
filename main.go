// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh_api/cli"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/clo_delegates"
	"github.com/boggydigital/vangogh/version"
	"github.com/boggydigital/wits"
	"os"
	"path/filepath"
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

	//TODO: Move this to vangogh_api as SetTempDir
	clo_delegates.SetTempDir(tempDir)
	//TODO: Rename this to SetStateDir
	vangogh_urls.ChRoot(stateDir)

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
		"digest":           cli.DigestHandler,
		"export":           cli.ExportHandler,
		"extract":          cli.ExtractHandler,
		"get-data":         cli.GetDataHandler,
		"get-downloads":    cli.GetDownloadsHandler,
		"get-images":       cli.GetImagesHandler,
		"get-videos":       cli.GetVideosHandler,
		"info":             cli.InfoHandler,
		"list":             cli.ListHandler,
		"owned":            cli.OwnedHandler,
		"version":          version.VersionHander,
		"vet":              cli.VetHandler,
		"search":           cli.SearchHandler,
		"serve":            cli.ServeHandler,
		"size":             cli.SizeHandler,
		"summary":          cli.SummaryHandler,
		"sync":             cli.SyncHandler,
		"tag":              cli.TagHandler,
		"update-downloads": cli.UpdateDownloadsHandler,
		"validate":         cli.ValidateHandler,
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

	return nil
}
