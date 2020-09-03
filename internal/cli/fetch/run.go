package fetch

import (
	"errors"
	"flag"
	"fmt"
	"github.com/boggydigital/vangogh/cmd"
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/auth"
	"github.com/boggydigital/vangogh/internal/gog/changes"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/mongocl"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

const (
	Cmd                  = "fetch"
	ProductsFlag         = "products"
	AccountProductsFlag  = "accountproducts"
	WishlistFlag         = "wishlist"
	Details              = "details"
	AllFlag              = "all"
	ProductsUsage        = "Fetch GOG.com products, values: all, games, movies"
	AccountProductsUsage = "Fetch GOG.com account products, values: all, games, movies"
	WishlistUsage        = "Fetch GOG.com wishlist products, values: all, games, movies"
	DetailsUsage         = "Fetch GOG.com details for account product by id"
	AllUsage             = "Fetch all GOG.com products, account and wishlist products"
	UnknownFlagValueMsg  = "Unknown value %s, supported values: all, games, movies.\n"
)

func parseMediaType(mts string) (media.Type, error) {
	mt := media.Parse(mts)
	if mt == media.Unknown {
		return mt, errors.New(fmt.Sprintf(UnknownFlagValueMsg, mts))
	}
	return mt, nil
}

func login(client *http.Client) error {
	loggedIn, err := auth.LoggedIn(client)
	if err != nil {
		return err
	}
	if !loggedIn {
		cfg, err := cfg.Current()
		if err != nil {
			return err
		}
		err = auth.LogIn(client, cfg.GOG.User, cfg.GOG.Pwd)
		if err != nil {
			return err
		}
	}
	return nil
}

func Run(httpClient *http.Client, mongoClient *mongo.Client, args []string) error {

	ctx, cancel, err := mongocl.Connect(mongoClient)
	defer mongocl.Disconnect(ctx, cancel, mongoClient)
	if err != nil {
		return err
	}

	changes.Init(mongoClient, ctx)

	fetchFlagSet := flag.NewFlagSet(Cmd, flag.ExitOnError)
	fetchProducts := fetchFlagSet.String(ProductsFlag, "", ProductsUsage)
	fetchAccountProducts := fetchFlagSet.String(AccountProductsFlag, "", AccountProductsUsage)
	fetchWishlist := fetchFlagSet.String(WishlistFlag, "", WishlistUsage)
	fetchDetails := fetchFlagSet.String(Details, "", DetailsUsage)
	fetchAll := fetchFlagSet.Bool(AllFlag, false, AllUsage)

	fetchFlagSet.Parse(args[2:])

	if *fetchAll {
		*fetchProducts = "all"
		*fetchAccountProducts = "all"
		*fetchWishlist = "all"
	}

	fetchDeps := cmd.FetchDeps{
		HttpClient:  httpClient,
		MongoClient: mongoClient,
		Ctx:         ctx,
		Media:       media.Unknown,
		Product:     "",
		Collection:  "",
	}

	if *fetchProducts != "" {
		mt, err := parseMediaType(*fetchProducts)
		if err != nil {
			return err
		}
		fetchDeps.Media = mt
		fetchDeps.Collection = mongocl.ProductsCollection
		fetchDeps.Product = "products"
		if err := cmd.FetchProducts(fetchDeps); err != nil {
			return err
		}
	}

	if *fetchAccountProducts != "" {
		mt, err := parseMediaType(*fetchAccountProducts)
		if err != nil {
			return err
		}
		if err := login(httpClient); err != nil {
			return err
		}
		fetchDeps.Media = mt
		fetchDeps.Collection = mongocl.AccountProductsCollection
		fetchDeps.Product = "account products"
		if err := cmd.FetchAccountProducts(fetchDeps); err != nil {
			return err
		}
	}

	if *fetchWishlist != "" {
		mt, err := parseMediaType(*fetchWishlist)
		if err != nil {
			return err
		}
		if err := login(httpClient); err != nil {
			return err
		}
		fetchDeps.Media = mt
		fetchDeps.Collection = mongocl.WishlistCollection
		fetchDeps.Product = "wishlist products"
		if err := cmd.FetchWishlist(fetchDeps); err != nil {
			return err
		}
	}

	if *fetchDetails != "" {
		fmt.Println("vangogh fetch details", *fetchDetails)
	}

	if *fetchProducts == "" &&
		*fetchAccountProducts == "" &&
		*fetchWishlist == "" &&
		*fetchDetails == "" {
		fmt.Println("vangogh fetch - need to specify what product , accountproduct, wishlist media or details id")
	}

	return nil
}
