package pcgw_data

import (
	"bufio"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"io"
	"maps"
	"slices"
	"strings"
)

const (
	txtExt            = ".txt"
	curlyBracesPfx    = "{{"
	curlyBracesSfx    = "}}"
	gameDataPfx       = "Game data/"
	gameDataConfigPfx = gameDataPfx + "config"
	gameDataSavesPfx  = gameDataPfx + "saves"
)

func GetRaw(pcgwGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.PcgwRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, txtExt)
	if err != nil {
		return err
	}

	gra.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwRaw(), kvRaw, gra, force); err != nil {
		return err
	}

	return ReduceRaw(pcgwGogIds, kvRaw)
}

func ReduceRaw(pcgwGogIds map[string][]string, kvRaw kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwRaw

	rra := nod.NewProgress(" reducing %s...", dataType)
	defer rra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.PcgwRawProperties()...)

	rra.TotalInt(len(pcgwGogIds))

	for pcgwPageId, gogIds := range pcgwGogIds {
		if !kvRaw.Has(pcgwPageId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, pcgwPageId))
			continue
		}

		if err = reduceRawProduct(gogIds, pcgwPageId, kvRaw, rawReductions); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
}

func reduceRawProduct(gogIds []string, pcgwPageId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcRaw, err := kvRaw.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	gameDataLines, err := filterGameData(rcRaw)
	if err != nil {
		return err
	}

	gdConfig := parseGameDataConfig(gameDataLines...)
	gdSaves := parseGameDataSaves(gameDataLines...)

	gdConfig = filterGameConfigSaves(gdConfig, gdSaves)

	for _, gogId := range gogIds {

		for os, cls := range gdConfig {
			if len(cls) == 0 {
				continue
			}
			os = strings.TrimSpace(os)
			switch os {
			case "DOS":
				piv[vangogh_integration.DosConfigProperty][gogId] = cls
			case "Mac OS":
				fallthrough
			case "OS X":
				piv[vangogh_integration.MacOsConfigProperty][gogId] = cls
			case "Linux":
				piv[vangogh_integration.LinuxConfigProperty][gogId] = cls
			case "Windows":
				piv[vangogh_integration.WindowsConfigProperty][gogId] = cls
			}
		}

		for os, sls := range gdSaves {
			if len(sls) == 0 {
				continue
			}
			os = strings.TrimSpace(os)
			switch os {
			case "DOS":
				piv[vangogh_integration.DosSavesProperty][gogId] = sls
			case "Mac OS":
				fallthrough
			case "OS X":
				piv[vangogh_integration.MacOsSavesProperty][gogId] = sls
			case "Linux":
				piv[vangogh_integration.LinuxSavesProperty][gogId] = sls
			case "Windows":
				piv[vangogh_integration.WindowsSavesProperty][gogId] = sls
			}
		}

	}

	return nil
}

func filterGameData(rcRaw io.Reader) ([]string, error) {
	gameData := make([]string, 0)

	scanner := bufio.NewScanner(rcRaw)
	for scanner.Scan() {
		if line := scanner.Text(); strings.HasPrefix(line, curlyBracesPfx+gameDataPfx) {
			gameData = append(gameData, line)
		}
	}

	if err := scanner.Err(); err != nil {
		return nil, err
	}

	return gameData, nil
}

func parseGameData(pfx string, lines ...string) map[string][]string {

	gameData := make(map[string][]string)

	for _, line := range lines {
		if !strings.Contains(line, curlyBracesPfx+pfx) {
			continue
		}
		line = strings.Trim(line, curlyBracesPfx+curlyBracesSfx)
		line = strings.TrimPrefix(line, pfx)
		line = strings.Replace(line, "{{p|", "$", -1)
		line = strings.Replace(line, "{{P|", "$", -1)
		line = strings.Replace(line, "}}", "", -1)
		if parts := strings.Split(line, "|"); len(parts) > 2 {
			for _, p := range parts[2:] {
				if p != "" {
					gameData[parts[1]] = append(gameData[parts[1]], p)
				}
			}
		}
	}

	return gameData
}

func parseGameDataConfig(lines ...string) map[string][]string {
	return parseGameData(gameDataConfigPfx, lines...)
}

func parseGameDataSaves(lines ...string) map[string][]string {
	return parseGameData(gameDataSavesPfx, lines...)
}

func filterGameConfigSaves(config, saves map[string][]string) map[string][]string {
	filteredConfig := make(map[string][]string)

	for os, cls := range config {
		for _, cl := range cls {
			if slices.Contains(saves[os], cl) {
				continue
			}
			filteredConfig[os] = append(filteredConfig[os], cl)
		}
	}

	return filteredConfig
}
