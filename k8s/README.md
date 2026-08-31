# Kubernetes deployment

Deploys the three modern apps (ASP.NET Core MVC, Razor Pages, CoreWCF) and SQL Server 2022 into the
`car-catalog` namespace. The MVC deployment is the only one that migrates and seeds the shared
database; the other two wait for the readiness check `catalog-database` to see a seeded schema.

The `sa` password is not stored in the repository. Create the secret before applying the manifests:

```bash
PASSWORD='<strong password>'
kubectl create namespace car-catalog
kubectl create secret generic catalog-sql \
  --namespace car-catalog \
  --from-literal=sa-password="$PASSWORD" \
  --from-literal=connection-string="Server=sql;Database=CarCatalog;User Id=sa;Password=$PASSWORD;TrustServerCertificate=True"
```

Build and load the images, then apply:

```bash
docker build -t carcatalog/web:dev -f modern/src/CarCatalog.Web/Dockerfile .
docker build -t carcatalog/razorpages:dev -f modern/src/CarCatalog.RazorPages/Dockerfile .
docker build -t carcatalog/wcf:dev -f modern/src/CarCatalog.WcfService/Dockerfile .

kind create cluster --name carcatalog
kind load docker-image --name carcatalog carcatalog/web:dev carcatalog/razorpages:dev carcatalog/wcf:dev

kubectl apply -k k8s
kubectl wait --namespace car-catalog --for=condition=available --timeout=10m deployment --all
```

The ingress rules expect an ingress controller and the hosts `mvc.carcatalog.local`,
`pages.carcatalog.local` and `wcf.carcatalog.local`. Without a controller, reach the apps with
`kubectl port-forward`:

```bash
kubectl port-forward --namespace car-catalog service/web 8080:80
kubectl port-forward --namespace car-catalog service/razorpages 8081:80
kubectl port-forward --namespace car-catalog service/wcf 8082:80
```
