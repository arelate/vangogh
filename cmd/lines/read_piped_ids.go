package lines

import (
	"bufio"
	"github.com/boggydigital/gost"
	"os"
	"strings"
)

func ReadPipedIds() (gost.StrSet, error) {
	idSet := gost.NewStrSet()
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		line := scanner.Text()
		if strings.HasPrefix(line, " ") {
			continue
		}
		tokens := strings.Split(line, " ")
		if len(tokens) < 1 {
			continue
		}

		idSet.Add(tokens[0])
	}
	if err := scanner.Err(); err != nil {
		return idSet, err
	}
	return idSet, nil
}
