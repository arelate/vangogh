package lines

import (
	"bufio"
	"os"
	"strings"
)

const commentPrefix = "//"

func Read(filepath string) []string {
	lines := make([]string, 0)
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
		lines = append(lines, scanner.Text())
	}

	return lines
}
