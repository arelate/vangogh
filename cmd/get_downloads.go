package cmd

import (
	"fmt"
)

func GetDownloads(ids map[string]bool, os []string, lang []string, all bool) error {
	fmt.Printf("get %s, %s downloads for %v\n", os, lang, ids)
	return nil
}
