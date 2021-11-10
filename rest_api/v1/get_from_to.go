package v1

import (
	"fmt"
	"strconv"
	"strings"
)

func getFromTo(numbersRange string) (int, int, error) {
	from, to := 0, 0
	var err error

	numbersStr := strings.Split(numbersRange, "-")

	if len(numbersStr) != 2 {
		return -1, -1, fmt.Errorf("invalid range %s, expected from-to", numbersRange)
	}
	if from, err = strconv.Atoi(numbersStr[0]); err != nil {
		return from, -1, fmt.Errorf("invalid `from` value %s, expected index", numbersStr[0])
	}
	if to, err = strconv.Atoi(numbersStr[1]); err != nil {
		return from, to, fmt.Errorf("invalid `to` value %s, expected index", numbersStr[1])
	}
	if from > to {
		return from, to, fmt.Errorf("`to` should be more or equal to `from`")
	}

	return from, to, nil
}
