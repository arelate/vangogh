package main

import (
	"bytes"
	_ "embed"
	"log"
	"net/url"
	"os"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/clo_delegates"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
)

var (
	//go:embed "cli-commands.txt"
	cliCommands []byte
	//go:embed "cli-help.txt"
	cliHelp []byte
)

const debugParam = "debug"

func main() {

	nod.EnableStdOutPresenter()

	vsa := nod.Begin("vangogh is serving your DRM-free needs")
	defer vsa.Done()

	if err := vangogh_integration.InitPathways(); err != nil {
		log.Fatalln(nod.Error(err))
	}

	defs, err := clo.Load(
		bytes.NewBuffer(cliCommands),
		bytes.NewBuffer(cliHelp),
		clo_delegates.FuncMap)
	if err != nil {
		log.Fatalln(nod.Error(err))
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"backup":                     cli.BackupHandler,
		"cleanup":                    cli.CleanupHandler,
		"dehydrate":                  cli.DehydrateHandler,
		"generate-missing-checksums": cli.GenerateMissingChecksumsHandler,
		"get-data":                   cli.GetDataHandler,
		"get-description-images":     cli.GetDescriptionImagesHandler,
		"get-downloads":              cli.GetDownloadsHandler,
		"get-images":                 cli.GetImagesHandler,
		"get-binaries":               cli.GetBinariesHandler,
		"get-summary":                cli.GetSummaryHandler,
		"get-video-metadata":         cli.GetVideoMetadataHandler,
		"import-cookies":             cli.ImportCookiesHandler,
		"migrate-data":               cli.MigrateDataHandler,
		"process-queue":              cli.ProcessQueueHandler,
		"reduce":                     cli.ReduceHandler,
		"relayout-downloads":         cli.RelayoutDownloadsHandler,
		"serve":                      cli.ServeHandler,
		"size":                       cli.SizeHandler,
		"summarize":                  cli.SummarizeHandler,
		"sync":                       cli.SyncHandler,
		"update-downloads":           cli.UpdateDownloadsHandler,
		"users":                      cli.UsersHandler,
		"validate":                   cli.ValidateHandler,
		"version":                    cli.VersionHandler,
	})

	if err = defs.AssertCommandsHaveHandlers(); err != nil {
		log.Fatalln(nod.Error(err))
	}

	var u *url.URL
	u, err = defs.Parse(os.Args[1:])
	if err != nil {
		log.Fatalln(err)
	}

	if q := u.Query(); q.Has(debugParam) {
		logger, err := nod.EnableFileLogger(u.Path, vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Logs))
		if err != nil {
			log.Fatalln(err)
		}
		defer logger.Close()
	}

	if err = defs.Serve(u); err != nil {
		log.Fatalln(nod.Error(err))
	}
}
