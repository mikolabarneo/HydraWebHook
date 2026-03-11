# HydraWebHook

* Set up
1. Install cdr for DNSEndpoint ```kubectl apply -f dnsendpoints.externaldns.k8s.io.yaml```
1. Initialize namespace, serviceAccount, clusterRole, clusterRoleBinding using ```kubectl apply -f external-dns-init.yaml``` 
2. Encode your Hydra API URI, TokenName and TokenPass like ```echo -n "https://some.hydra/api/ipam/" | base64``` mind -n to exclude carriage return after the string
3. Fill in base64 strings into external-dns-hydra-secret.yaml
4. ```kubectl apply -f external-dns-hydra-secret.yaml```
5. ```kubectl apply -f external-dns-with-hydra-webhook.yaml```

As a result pod with 2 containers (one of which is our webhook sidecar) should start up

* Reading logs
