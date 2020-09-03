package main

import (
	"context"
	"encoding/json"
	"fmt"
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/products"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"io/ioutil"
	"log"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"path/filepath"
	"strconv"
	"time"
)

const (
	logFile  = "vangogh.log"
	checkLog = "Please check $TMPDIR/" + logFile + " for details."
)

type RemoteOrigin interface {
	Fetch(id int) ([]byte, error)
	Copy(dest *LocalDestination) error
}

type LocalDestination interface {
	Connect() error
	Insert(data interface{}) error
	Disconnect() error
}

type Origin struct {
	HttpClient *http.Client
	URL        *url.URL
}

type Destination struct {
	MongoClient *mongo.Client
	Ctx         context.Context
	DB          string
	Collection  string
}

type ProductsDestination struct {
	Destination
}

func NewOrigin(httpClient *http.Client, url *url.URL) *Origin {
	return &Origin{
		HttpClient: httpClient,
		URL:        url,
	}
}

func NewPageOrigin(httpClient *http.Client, url *url.URL) *PageOrigin {
	return &PageOrigin{
		Origin{
			HttpClient: httpClient,
			URL:        url,
		},
	}
}

func NewProductPageOrigin(httpClient *http.Client, mt media.Type) *ProductPageOrigin {
	return &ProductPageOrigin{
		PageOrigin{
			Origin{
				HttpClient: httpClient,
				URL:        urls.ProductsPageURL(mt),
			},
		},
	}
}

func NewProductsDestination(mongoClient *mongo.Client) *ProductsDestination {
	return &ProductsDestination{
		Destination{
			MongoClient: mongoClient,
			DB:          "vangogh",
			Collection:  "products",
		},
	}
}

func (origin *Origin) Fetch(id int) (*[]byte, error) {

	resp, _ := origin.HttpClient.Get(origin.URL.String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	return &respBody, err
}

type PageOrigin struct {
	Origin
}

type ProductPageOrigin struct {
	PageOrigin
}

func (pageOrigin *PageOrigin) Fetch(page int) (*[]byte, error) {

	q := pageOrigin.URL.Query()
	q.Add("page", strconv.Itoa(page))
	pageOrigin.URL.RawQuery = q.Encode()

	return pageOrigin.Origin.Fetch(page)
}

func (origin *ProductPageOrigin) Copy(page int, dest LocalDestination) (totalPages int, err error) {

	bytes, _ := origin.Fetch(page)
	//productPage := products.ProductPage{}
	var pageData map[string]interface{}
	_ = json.Unmarshal(*bytes, &pageData)

	_ = dest.Connect()
	for _, p := range pageData["products"].([]interface{}) {
		fmt.Print(".")
		pjson, _ := json.Marshal(p)
		_ = dest.Insert(pjson)
	}
	_ = dest.Disconnect()

	return int(pageData["totalPages"].(float64)), nil
}

func (dest *Destination) Connect() error {
	dest.Ctx = context.Background()
	err := dest.MongoClient.Connect(dest.Ctx)
	if err != nil {
		return err
	}
	return dest.MongoClient.Ping(dest.Ctx, nil)
}

func (dest *Destination) Disconnect() error {
	return dest.MongoClient.Disconnect(dest.Ctx)
}

func (dest *Destination) Insert(data interface{}) error {

	col := dest.MongoClient.Database(dest.DB).Collection(dest.Collection)

	//h, err := hash.Sha256(data)
	//if err != nil {
	//	return err
	//}

	//chg, err := changes.Get(colName, id)
	//if err != nil {
	//	return err
	//}
	//
	//switch chg.Hash {
	//case "":
	_, err := col.InsertOne(dest.Ctx, data)
	if err != nil {
		return err
	}
	//	return changes.Insert(colName, changes.New(id, h))
	//case h:
	//	// data unchanged. Do nothing.
	//default:
	//	_, err = col.ReplaceOne(ctx, bson.M{"_id": id}, data)
	//	if err != nil {
	//		return err
	//	}
	//
	//	return changes.Replace(colName, id, chg.Update(h))
	//}
	return nil
}

func (pdest *ProductsDestination) Insert(pjson interface{}) error {
	var product products.Product
	_ = json.Unmarshal(pjson.([]byte), &product)

	return pdest.Destination.Insert(product)
}

func main() {

	f, err := os.Create(filepath.Join(os.TempDir(), logFile))
	log.SetOutput(f)
	log.SetFlags(log.Ldate | log.Ltime | log.Lshortfile)
	log.Println("Started new session")

	cookies, err := session.Load()
	if err != nil {
		fmt.Println("Cannot load session cookies.", checkLog)
		log.Fatalln(err)
	}

	jar, err := cookiejar.New(nil)
	if err != nil {
		fmt.Println("Cannot create cookie jar.", checkLog)
		log.Fatalln(err)
	}

	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if jar != nil && cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	httpClient := &http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	gameProductsOrigin := NewProductPageOrigin(httpClient, media.Game)

	conf, _ := cfg.Current()
	mongoClient, err := mongo.NewClient(
		options.Client().ApplyURI(conf.Mongo.Conn))
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	productsDest := NewProductsDestination(mongoClient)

	tp, _ := gameProductsOrigin.Copy(1, productsDest)
	fmt.Println(tp)

	os.Exit(1)

	//mongoClient, err := mongocl.New()
	//if err != nil {
	//	fmt.Println("Cannot create MongoDB client.", checkLog)
	//	log.Fatalln(err)
	//}
	//
	//if err := cli.Run(httpClient, mongoClient, os.Args); err != nil {
	//	fmt.Println("Application has encountered an error.", checkLog)
	//	log.Fatalln(err)
	//}
	//
	//err = session.Save(httpClient.Jar.Cookies(gogHost))
	//if err != nil {
	//	fmt.Println("Cannot save session cookies.", checkLog)
	//	log.Fatalln(err)
	//}
}
