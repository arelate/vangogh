package internal

import (
	"bufio"
	"os"
	"strings"
)

const commentPrefix = "//"

func ReadLines(filepath string) map[string]bool {
	lines := make(map[string]bool, 0)
	if filepath == "" {
		return lines
	}

	file, err := os.Open(filepath)
	if err != nil {
		return lines
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		line := scanner.Text()
		if strings.HasPrefix(line, commentPrefix) {
			continue
		}
		lines[scanner.Text()] = true
	}

	return lines
}
