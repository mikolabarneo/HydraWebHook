# HydraWebHook

* Setup

1. Install cdr for DNSEndpoint ```kubectl apply -f dnsendpoints.externaldns.k8s.io.yaml```
1. Initialize namespace, serviceAccount, clusterRole, clusterRoleBinding using ```kubectl apply -f external-dns-init.yaml``` 
2. Replace placeholders in external-dns-hydra-secret.yaml with your data or use your secret from hydra certificate application
3. Write down the Hydra server Url (ended with '/') as an ENV variable HYDRA_URI text to external-dns-with-hydra-webhook.yaml
4. ```kubectl apply -f external-dns-hydra-secret.yaml```
5. ```kubectl apply -f external-dns-with-hydra-webhook.yaml```

As a result pod with 2 containers (one of which is our webhook sidecar) should start up

* CRDs and services

The args section of the external-dns container in ```external-dns-with-hydra-webhook.yaml``` *contains --source*

if *- --source=crd* present, the application will try to register/modify records set up using DNSEndpoint artifacts defined on the entire cluster, for the example see ```external-dns-dnsendpoint-{a|cname}-example.yaml```
if *- --source=service* present, the application will try to register/modify A records automatically for k8s service of type LoadBalancer (mostly contour envoy that have ExternalIP defined). But you need to add annotation to populate the hostname for the A record. 
```
 metadata:
  annotations:
    external-dns.alpha.kubernetes.io/hostname: csst-cluster-ingress.shore.ox.ac.uk
```
if *- --source=contour-httpproxy* present, no additional annotations needed, in this case the application will try to register A record for each httpproxy entry