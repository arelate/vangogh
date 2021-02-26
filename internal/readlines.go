package internal

import (
	"bufio"
	"os"
)

func ReadLines(filepath string) []string {
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
		lines = append(lines, scanner.Text())
	}

	return lines
}
