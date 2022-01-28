package main

import (
	"github.com/boggydigital/wits"
	"os"
)

const userDirectoriesFilename = "directories.txt"

func readUserDirectories() error {
	if _, err := os.Stat(userDirectoriesFilename); os.IsNotExist(err) {
		return nil
	}

	udFile, err := os.Open(userDirectoriesFilename)
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
